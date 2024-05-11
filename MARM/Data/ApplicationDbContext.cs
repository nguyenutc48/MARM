using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Xml.Linq;
namespace MARM.Data;

public class ApplicationDbContext : DbContext
{
    private DbSet<NavalUnit> NavalUnits { get; set; }
    private DbSet<Mission> Missions { get; set; }
    private DbSet<BoatUnitMission> BoatUnitMissions { get; set; }
    private DbSet<BoatUnitShot> BoatUnitShots { get; set; }
    private DbSet<AppConfig> AppConfigs { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public async Task<IEnumerable<NavalUnit>> GetNavalUnits(Guid parentId) => await NavalUnits.Where(u => u.ParentId == parentId).ToListAsync();

    public async Task<Result<NavalUnit>> CreateNavalUnit(Guid parentId, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new Result<NavalUnit>()
            {
                IsSuccess = false,
                Message = $"Tên đơn vị không được để trống",
            };
        }

        if (await NavalUnits.AnyAsync(u => u.ParentId == parentId && u.Name == name))
        {
            return new Result<NavalUnit>()
            {
                IsSuccess = false,
                Message = $"Tên đơn vị {name} đã tồn tại",
            };
        }

        var entry = await NavalUnits.AddAsync(new NavalUnit { ParentId = parentId, Name = name, });
        await SaveChangesAsync();

        return new Result<NavalUnit>()
        {
            IsSuccess = true,
            Message = $"Tạo thành công đơn vị {name}",
            Value = new NavalUnit { ParentId = parentId, Name = name, Id = entry.Entity.Id },
        };
    }

    public async Task<Result> DeleteNavalUnit(Guid id)
    {
        var unit = await NavalUnits.FindAsync(id);
        if (unit == null) return new Result() { IsSuccess = false, Message = $"Không tìm thấy dữ liệu đơn vị cần xóa, ID = {id}", };

        var children = await NavalUnits.Where(u => u.ParentId == unit.Id).ToListAsync();

        if (children.Any())
        {
            foreach (var child in children)
            {
                var result = await DeleteNavalUnit(child.Id);
                if (!result.IsSuccess)
                {
                    return new Result() { IsSuccess = false, Message = $"Lỗi xóa đơn vị trực thuộc {child.Name}.\n Message: {result.Message}", };
                }
            }
        }

        NavalUnits.Remove(unit);
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = $"Xóa thành công đơn vị {unit.Name}", };
    }

    public async Task<IEnumerable<Mission>> GetNavalMissions(Guid id)
    {
        var missions = await Missions.Where(u => u.NavalUnitId == id).ToListAsync();
        foreach (var mission in missions)
        {
            mission.BoatCount = await BoatUnitMissions.CountAsync(b => b.MissionId == mission.Id);
        }

        return missions;
    }

    public async Task<Mission?> GetNavalMissionById(Guid id) => await Missions.FindAsync(id);

    public async Task<Result<Mission>> CreateNewNavalMission(Guid unitId, string name, DateTime startAt, string note)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new Result<Mission>()
            {
                IsSuccess = false,
                Message = $"Tên nhiệm vụ không được để trống",
            };
        }

        if (await Missions.AnyAsync(m => m.NavalUnitId == unitId && m.Name == name))
        {
            return new Result<Mission> { IsSuccess = false, Message = "Tên nhiệm vụ đã tồn tại", };
        }

        var entry = await Missions.AddAsync(new Mission()
        {
            NavalUnitId = unitId,
            Name = name,
            State = Enums.MissionState.Create,
            CreateAt = DateTime.Now,
            StartAt = startAt,
            ModifiedAt = DateTime.Now,
            Note = note,
        });

        await SaveChangesAsync();
        return new Result<Mission> { IsSuccess = true, Message = $"Tạo nhiệm vụ {name} thành công.", Value = entry.Entity, };
    }

    public async Task<Result> DeleteNavalMission(Guid id)
    {
        var mission = await Missions.FindAsync(id);
        if (mission == null) return new Result() { IsSuccess = false, Message = $"Không tìm thấy dữ liệu nhiệm vụ cần xóa, ID = {id}", };

        Missions.Remove(mission);
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = $"Xóa thành công nhiệm vụ {mission.Name}", };
    }

    private static readonly Random rnd = new();

    public async Task<IEnumerable<BoatUnitMissionResult>> GetAllBoatUnitMissions()
    {
        return await BoatUnitMissions.Select((BoatUnitMission,index) => new BoatUnitMissionResult
        {
            Index = index+1,
        }).ToListAsync();
    }
    public async Task<List<BoatUnitMissionExportResult>> GetAllBoatUnitMissions(DateTime? dateTime)
    {
        DateTime date = dateTime ?? DateTime.Now;
        List<BoatUnitMissionExportResult> boatUnitMissionExportResults = new List<BoatUnitMissionExportResult>();
        var query = from boatUnit in BoatUnitMissions
                    join boatShotResult in BoatUnitShots on boatUnit.Id equals boatShotResult.BoatUnitId
                    where boatShotResult.Time.Date == date.Date
                    select new
                    {
                        boatUnit.Id,
                        boatUnit.Name,
                        boatShotResult.Time,
                        boatShotResult.Position,
                        boatUnit.Note
                    };
        var result = await query.GroupBy(g=>g.Id).ToListAsync();
        //Console.WriteLine(JsonConvert.SerializeObject(result));
        int index = 1;
        foreach (var newBoat in result)
        {
            BoatUnitMissionExportResult boatUnitMissionExportResult = new BoatUnitMissionExportResult();
            var boat = await BoatUnitMissions.FirstOrDefaultAsync(b=>b.Id == newBoat.Key);
            boatUnitMissionExportResult.Index = index;
            boatUnitMissionExportResult.BoatName = boat.Name;
            boatUnitMissionExportResult.Note = boat.Note;
            int totalShot = 0;
            int[] shotCounts = new int[16];
            foreach (var item in newBoat)
            {
                totalShot++;
                shotCounts[item.Position]++;
            }
            boatUnitMissionExportResult.ShotTotal = totalShot;
            boatUnitMissionExportResult.ShotTime = newBoat.First().Time.ToString("dd/MM/yyyy");

            var indexes = shotCounts.Select((value, index) => new { value, index })
               .Where(item => item.value != 0)
               .Select(item => item.index+1);

            boatUnitMissionExportResult.ShotPosition = string.Join(",", indexes);
            
            boatUnitMissionExportResults.Add(boatUnitMissionExportResult);
            index++;
        }
        //Console.WriteLine(JsonConvert.SerializeObject(boatUnitMissionExportResults));
        return boatUnitMissionExportResults;
    }

    public async Task<IEnumerable<BoatUnitMission>> GetBoatUnitMissions(Guid id)
    {
        var boards = await BoatUnitMissions.Where(u => u.MissionId == id).ToListAsync();
        foreach (var board in boards)
        {
            var boatUnitShots = await BoatUnitShots.Where(s=>s.BoatUnitId == board.Id).ToListAsync();

            foreach (var boatUnitShot in boatUnitShots)
            {
                board.ShotTotal++;
                board.ShotCounts[boatUnitShot.Position] ++;
            }
        }

        return boards;
    }

    public async Task<Result<BoatUnitMission>> CreateNewBoatUnitMission(Guid missionId, string name, string note)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new Result<BoatUnitMission>()
            {
                IsSuccess = false,
                Message = $"Tên đơn vị tàu không được để trống",
            };
        }

        if (await BoatUnitMissions.AnyAsync(b => b.MissionId == missionId && b.Name == name))
        {
            return new Result<BoatUnitMission>
            {
                IsSuccess = false,
                Message = "Tên đơn vị tàu đã tồn tại",
            };
        }

        var entry = await BoatUnitMissions.AddAsync(new BoatUnitMission()
        {
            MissionId = missionId,
            Name = name,
            Note = note,
        });

        await SaveChangesAsync();
        return new Result<BoatUnitMission> { IsSuccess = true, Message = "", Value = entry.Entity, };
    }

    public async Task<Result> DeleteBoatUnitMission(Guid id)
    {
        var boat = await BoatUnitMissions.FindAsync(id);
        if (boat == null) return new Result() { IsSuccess = false, Message = $"Không tìm thấy dữ liệu đơn vị tàu cần xóa, ID = {id}", };

        BoatUnitMissions.Remove(boat);
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = $"Xóa thành công đơn vị tàu {boat.Name}", };
    }

    public async Task<Result> UpdateBoatUnitMissionInfo(Guid id, string name, string note)
    {
        if(string.IsNullOrEmpty(name)) return new Result() { IsSuccess = false, Message = "Tên đơn vị tàu không được để trống", };

        var boat = await BoatUnitMissions.FindAsync(id);
        if (boat == null) return new Result() { IsSuccess = false, Message = $"Không tìm thấy dữ liệu đơn vị tàu cần update, ID = {id}", };

        if(await BoatUnitMissions.AnyAsync(b => b.MissionId == boat.MissionId && b.Name == name && b.Id != boat.Id))
        {
            return new Result() { IsSuccess = false, Message = $"Tên đơn vị tàu {name} bị trùng", };
        }

        boat.Name = name;
        boat.Note = note;
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = "", };
    }

    public async Task<IEnumerable<BoatUnitShot>> GetBoatUnitShots(Guid id) => await BoatUnitShots.Where(s => s.BoatUnitId == id).ToListAsync();

    public async Task<Result<BoatUnitShot>> CreateBoatUnitShot(Guid boatId, int position)
    {
        var entry = await BoatUnitShots.AddAsync(new BoatUnitShot()
        {
            BoatUnitId = boatId,
            Position = position,
            Time = DateTime.Now,
        });

        await SaveChangesAsync();
        return new Result<BoatUnitShot> { IsSuccess = true, Message = "", Value = entry.Entity, };
    }

    public async Task<AppConfig> GetConfigAsync()
    {
        var config = new AppConfig();
        config = await AppConfigs.FirstOrDefaultAsync();
        if (config == null) return new AppConfig();
        return config;
    }

    public async Task<Result> CreateConfigAsync(AppConfig config)
    {
        await AppConfigs.AddAsync(config);
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = "", };
    }

    public async Task<Result> UpdateConfigAsync(AppConfig config)
    {
        AppConfigs.Update(config);
        await SaveChangesAsync();
        return new Result() { IsSuccess = true, Message = "", };
    }

}

