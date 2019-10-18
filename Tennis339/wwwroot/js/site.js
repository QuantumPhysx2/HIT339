// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification for details on configuring this project to bundle and minify static web assets.

var toggler = document.querySelector(".navbar-toggler"); 
var menu = document.querySelector(".navbar-collapse");

toggler.addEventListener("click", function () {

    if (menu.style.display == "unset") {
        menu.style.display = "none";
    } 
    else {
        menu.style.display = "unset";
    }
});
