// wwwroot/js/scripts.js

window.addClassToBody = function (className) {
    document.body.classList.add(className);
};

window.removeClassFromBody = function (className) {
    document.body.classList.remove(className);
};

function downloadFile(fileBytes, fileName) {
    // Create a Blob from the file data (PDF content)
    var blob = new Blob([fileBytes], { type: "application/pdf" });

    // Create a URL for the Blob object
    var url = window.URL.createObjectURL(blob);

    // Create an invisible download link
    var link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    // Programmatically click the link to trigger the download
    link.click();

    // Clean up by revoking the object URL after the download is triggered
    window.URL.revokeObjectURL(url);
}

