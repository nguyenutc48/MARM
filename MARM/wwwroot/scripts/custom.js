window.showSaveFileDialog = function () {
    return new Promise((resolve, reject) => {
        const input = document.createElement('input');
        input.type = 'file';
        input.accept = '.xlsx'; // Chỉ cho phép chọn file Excel
        input.onchange = (event) => {
            const files = event.target.files;
            if (files.length > 0) {
                resolve(files[0].path); // Trả về đường dẫn tệp đã chọn
            } else {
                reject('No file selected');
            }
        };
        input.click();
    });
};

window.showSaveFileDialog1 = function (defaultFileName) {
    return new Promise((resolve, reject) => {
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = '#';
        a.download = defaultFileName || 'export.xlsx';
        a.onclick = () => {
            resolve(a.download);
            window.URL.revokeObjectURL(a.href);
        };
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    });
};

window.DownloadFile = function (filePath) {
    // Gửi yêu cầu xuất file đến backend
    fetch(`/api/export/excel?filePath=${encodeURIComponent(filePath)}`, {
        method: 'GET'
    })
        .then(response => response.blob())
        .then(blob => {
            // Tạo một đường dẫn tạm thời
            const url = window.URL.createObjectURL(new Blob([blob]));
            // Tạo một thẻ <a> để tải xuống
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = filePath.substring(filePath.lastIndexOf('/') + 1);
            document.body.appendChild(a);
            a.click();
            // Xóa đường dẫn tạm thời
            window.URL.revokeObjectURL(url);
        })
        .catch(error => console.error('Error:', error));
}

window.DownloadExcel = function (data) {
    // Gửi yêu cầu xuất file Excel đến backend
    fetch('/api/export/excel', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => response.blob())
        .then(blob => {
            // Tạo một đường dẫn tạm thời
            const url = window.URL.createObjectURL(new Blob([blob]));
            // Tạo một thẻ <a> để tải xuống
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = 'output.xlsx'; // Tên file Excel
            document.body.appendChild(a);
            a.click();
            // Xóa đường dẫn tạm thời
            window.URL.revokeObjectURL(url);
        })
        .catch(error => console.error('Error:', error));
}

window.exportToExcel = function (content, fileName) {
    // Tạo một đường dẫn tạm thời
    var url = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + content;

    // Tạo một thẻ <a> để tải xuống
    var a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    // Xóa đường dẫn tạm thời
    window.URL.revokeObjectURL(url);
}

