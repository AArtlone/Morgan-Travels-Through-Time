$(document).ready(function () {
    // Personas references
    var titleOfCurrentPersona = document.getElementsByClassName("title");
    var personas = document.getElementsByClassName("persona");
    var previousPersona = document.getElementsByClassName("fa-caret-left")[0];
    var nextPersona = document.getElementsByClassName("fa-caret-right")[0];

    var _currentPersonaIndex = 0;

    togglePersona();

    previousPersona.addEventListener("click", function() {
        _currentPersonaIndex--;

        togglePersona();
    });

    nextPersona.addEventListener("click", function() {
        _currentPersonaIndex++;

        togglePersona();
    });

    function togglePersona() {
        if (_currentPersonaIndex < 0) {
            _currentPersonaIndex = personas.length - 1;
        }
        if (_currentPersonaIndex > personas.length - 1) {
            _currentPersonaIndex = 0;
        }

        for (var i = 0; i < personas.length; i++) {
            if (i == _currentPersonaIndex) {
                personas[i].style.display = "block";
                for (var j = 0; j < titleOfCurrentPersona.length; j++) {
                    var personaName = personas[i].getElementsByClassName("personaName")[0].textContent;

                    titleOfCurrentPersona[j].textContent = personaName.substring(5, personaName.length);
                }
            } else {
                personas[i].style.display = "none";
            }
        }
    }

    // Campaigns references
    var titleOfCurrentYear = document.getElementsByClassName("currentYear");
    
    // ********************
    // JQuery section
    // ********************
    
    // Navigation bar references
    var page = $(document);
    var personasLink = $(".personasLink");
    var personasSection = $("#personas");

    var campaignsLink = $(".campaignsLink");
    var campaignsSection = $("#campaigns");

    var contentLink = $(".contentLink");
    var contentSection = $("#content");

    var returnToTopButton = $(".returnToTop");
    var logoLink = $(".logo");
    var navList = $(".navList");

    personasLink.on("click", function () {
        scrollToSectionID("personas");
        toggleActiveLink(personasLink);
    });

    campaignsLink.on("click", function () {
        scrollToSectionID("campaigns");
        toggleActiveLink(campaignsLink);
    });

    contentLink.on("click", function () {
        scrollToSectionID("content");
        toggleActiveLink(contentLink);
    });

    returnToTopButton.on("click", function () {
        scrollToSectionID("wrapper");
        toggleActiveLink(personasLink);
    });

    logoLink.on("click", function () {
        scrollToSectionID("wrapper");
        toggleActiveLink(personasLink);
    });

    page.on("scroll", function () {
        var scrollFromTop = page.scrollTop();
        if (scrollFromTop < campaignsSection.offset().top - 200 &&
            scrollFromTop > 0) {
            toggleActiveLink(personasLink);
        } else if (scrollFromTop > personasSection.offset().top - 250 &&
            scrollFromTop < contentSection.offset().top - 250) {
            toggleActiveLink(campaignsLink);
        } else if (scrollFromTop > campaignsSection.offset().top) {
            toggleActiveLink(contentLink);
        }
    });

    function scrollToSectionID(id) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#" + id).offset().top -
                (("#" + id) == "#wrapper" ? $("#" + id).offset().top : 150)
        }, 1000);
    }

    function toggleActiveLink(buttonClicked) {
        for (var i = 0; i < navList.find("li").length; i++) {
            if (navList.find("li").eq(i).text() == buttonClicked.text()) {
                navList.find("li").eq(i).find("a").addClass("active");
            } else {
                navList.find("li").eq(i).find("a").removeClass("active");
            }
        }
    }
});
