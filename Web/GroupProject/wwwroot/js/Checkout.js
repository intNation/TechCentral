document.addEventListener("DOMContentLoaded", function () {
    var tabs = document.querySelectorAll(".resp-tabs-list li");
    var tabContents = document.querySelectorAll(".tab-content");

    tabContents.forEach((content, index) => {
        if (index !== 0) {
            content.style.display = "none";
        }
    });

    tabs.forEach((tab, index) => {
        tab.addEventListener("click", function () {
         
            tabs.forEach(t => t.classList.remove("active"));
           
            this.classList.add("active");

            tabContents.forEach(content => content.style.display = "none");
            tabContents[index].style.display = "block";
        });
    });
});