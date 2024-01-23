// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#myTable').DataTable();
});
$(document).ready(function () {
    $(".edit-icon").click(function () {
        window.location = $(this).attr("href"); // Redirect to link's URL when icon is clicked
        return false; // Prevent default link behavior (e.g. Page refresh)
    });
});
