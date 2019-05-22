$(document).ready(function () {
    // Personas references
    var titleOfCurrentPersona = document.getElementsByClassName("title");
    var personas = document.getElementsByClassName("persona");
    var previousPersona = document.getElementsByClassName("fa-caret-left")[0];
    var nextPersona = document.getElementsByClassName("fa-caret-right")[0];

    var currentPersonaIndex = 0;

    togglePersona();

    previousPersona.addEventListener("click", function () {
        currentPersonaIndex--;

        togglePersona();
    });

    nextPersona.addEventListener("click", function () {
        currentPersonaIndex++;

        togglePersona();
    });

    function togglePersona() {
        if (currentPersonaIndex < 0) {
            currentPersonaIndex = personas.length - 1;
        }
        if (currentPersonaIndex > personas.length - 1) {
            currentPersonaIndex = 0;
        }

        for (var i = 0; i < personas.length; i++) {
            if (i == currentPersonaIndex) {
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

    // Campaigns references
    // **********************************
    // Timeline-visualization references
    var campaignWrappers = document.getElementsByClassName("campaignWrapper");
    var currentYear = document.getElementsByClassName("currentYear")[0];
    var currentYearIndex = 0;
    var currentCampaign = document.getElementsByClassName("currentCampaign")[0];
    var currentCampaignIndex = 0;
    var buttonsForActivities = document.getElementsByClassName("ActivityButton");

    // **********************************
    // Timeline-selection references
    var previousYear = document.getElementsByClassName("previousYear")[0];
    var nextYear = document.getElementsByClassName("nextYear")[0];
    var previousCampaign = document.getElementsByClassName("previousCampaign")[0];
    var nextCampaign = document.getElementsByClassName("nextCampaign")[0];
    var monthlyButtons = document.getElementsByName("Month Button");

    // **********************************
    // Timeline-details references
    var titleOfSelectedActivity = document.getElementsByClassName("titleOfSelectedActivity")[0];
    var typeOfSelectedActivity = document.getElementsByClassName("typeOfSelectedActivity")[0];
    var descriptionOfSelectedActivity = document.getElementsByClassName("descriptionOfSelectedActivity")[0];

    var jsonData = {"Years":[{"Name":"2019","Campaigns":[{"Name":"First Title 1","Campaign":[{"Name":"Jun","Description":"The month of June 2019 marks the beginning of the pre-launch period. Starting from this month, the aim is to generate interest in our target audience, schools, via online and offline tools at our disposal. The tools selected for this period will be: social media, fliers, and business cards. This initial tools would require 11 flyers and 18 cards, for a budget of 29€.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Jul","Description":"The month of July 2019 will be the last available before the launch of the first title, and therefore will be dedicated, once again, to generating interest and to the last preparations. The only tool selected for this period is social media, and the budget is 0€.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Aug","Description":"August is the selected month for the launch. The game is to launch during the Bommen Berend event, as it shares the subject with the event. This allows for the product to be promoted alongside the celebration. The tools used in this period will be: television, social media, and promotional material. Tv advertisement, for 25000€, and promotional material (chocolate), for 1197,50€, make up a budget of 26198€.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Sep","Description":"The month following the launch will be dedicated to the promotion of the game. Such promotion will be achieved via discounts with reviews. The tool required for this month is social media, with a budget of 0€.","Activities":[{"Name":"School Start","Type":"Sales","Description":"School Start description"}]}]},{"Name":"First Title 2","Campaign":[{"Name":"Jun","Description":"The month of June 2019 marks the beginning of the pre-launch period. Starting from this month, the aim is to generate interest in our target audience, primary school children. As such, the tools chosen for this month are Youtube, social media and fliers. Youtube ads cost circa 10€ per day, therefore the final estimated budget is 311€, adding the cost of the fliers.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Jul","Description":"The month of July 2019 will be the last available for the pre-launch period. With the aim to generate interest in our target audience, primary school children, the tools chosen for this month are Youtube and social media. The cost of running ads on Youtube for a month amounts to 300€ and constitutes our budget for the month.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Aug","Description":"August is the selected month for the launch. The game is to launch during the Bommen Berend event, as it shares the subject with the event. This allows for the product to be promoted alongside the celebration. The tools used in this period will be: Youtube, television, social media, and promotional material. Youtube ads, for 300€, Tv advertisement, for 25000€, and promotional material (buttons and chocolate), for 1197,50€, make up a budget of 26558€.","Activities":[{"Name":"First Title","Type":"Launch","Description":"First Title description"},{"Name":"10% Off New Title","Type":"Promotion","Description":"10% Off New Title description"}]},{"Name":"Sep","Description":"The month following the launch will be dedicated to the promotion of the game. Such promotion will be achieved via discounts with reviews. The tool required for this month is once again Youtube, with a budget of 300€.","Activities":[{"Name":"School Start","Type":"Sales","Description":"School Start description"}]}]}]}]};

    // ******************
    // Years events
    previousYear.addEventListener("click", function () {
        currentYearIndex--;

        toggleYear();
    });

    nextYear.addEventListener("click", function () {
        currentYearIndex++;

        toggleYear();
    });

    function toggleYear() {
        if (currentYearIndex < 0) {
            currentYearIndex = jsonData.Years.length - 1;
        }
        if (currentYearIndex > jsonData.Years.length - 1) {
            currentYearIndex = 0;
        }

        currentYear.textContent = jsonData.Years[currentYearIndex].Name;
    }

    toggleCampaign();

    // ******************
    // Campaigns events
    previousCampaign.addEventListener("click", function () {
        currentCampaignIndex--;

        toggleCampaign();
    });

    nextCampaign.addEventListener("click", function () {
        currentCampaignIndex++;

        toggleCampaign();
    });

    function toggleCampaign() {
        if (currentCampaignIndex < 0) {
            currentCampaignIndex = jsonData.Years[currentYearIndex].Campaigns.length - 1;
        }
        if (currentCampaignIndex > jsonData.Years[currentYearIndex].Campaigns.length - 1) {
            currentCampaignIndex = 0;
        }

        // Showing the currently selected campaign
        for (var i = 0; i < jsonData.Years[currentYearIndex].Campaigns.length; i++) {
            if (i == currentCampaignIndex) {
                campaignWrappers[i].style.display = "inline-block";
            } else {
                campaignWrappers[i].style.display = "none";
            }
        }

        currentCampaign.textContent = "Campaign: " + jsonData.Years[currentYearIndex].Campaigns[currentCampaignIndex].Name;

        titleOfSelectedActivity.textContent = jsonData.Years[currentYearIndex].Campaigns[currentCampaignIndex].Campaign[0].Name;

        descriptionOfSelectedActivity.textContent = jsonData.Years[currentYearIndex].Campaigns[currentCampaignIndex].Campaign[0].Description;

        // Resetting the highlighted month
        for (var j = 0; j < monthlyButtons.length; j++) {
            if ($(monthlyButtons[j]).hasClass("active")) {
                $(monthlyButtons[j]).removeClass("active");
            }
        }
        for (var i = 0; i < campaignWrappers.length; i++) {
            if (campaignWrappers[i].style.display == "inline-block") {
                $(monthlyButtons[0 + (i * 4)]).addClass("active");
            }   
        }
    }

    // ******************
        // Campaign activities
        for (var i = 0; i < buttonsForActivities.length; i++) {
            buttonsForActivities[i].addEventListener("click", function () {
                for (var j = 0; j < jsonData.Years.length; j++) {
                    if (j == currentYearIndex) {
                        for (var k = 0; k < jsonData.Years[j].Campaigns.length; k++) {
                            if (k == currentCampaignIndex) {
                                for (var l = 0; l < jsonData.Years[j].Campaigns[k].Campaign.length; l++) {
                                    for (var a = 0; a < jsonData.Years[j].Campaigns[k].Campaign[l].Activities.length; a++) {
                                        var arrayOfNames = $(this).attr("name").split(" | ");
                                        var title = arrayOfNames[0];
                                        var type = arrayOfNames[1];
                                        
                                        if (jsonData.Years[j].Campaigns[k].Campaign[l].Activities[a].Name == title) {
                                            titleOfSelectedActivity.textContent = jsonData.Years[j].Campaigns[k].Campaign[l].Activities[a].Name;
    
                                            typeOfSelectedActivity.style.display = "block";

                                            resetActivityClasses();
                                            
                                            var newIcon;
                                            switch (type) {
                                                case "Launch":
                                                    $(this).addClass("activeGreenActivity");
                                                    newIcon = '<i class="fas fa-rocket"></i>';
                                                    break;
                                                case "Promotion":
                                                    $(this).addClass("activeBlueActivity");
                                                    newIcon = '<i class="fas fa-tv"></i>';
                                                    break;
                                                case "Sales":
                                                    $(this).addClass("activeOrangeActivity");
                                                    newIcon = '<i class="fas fa-money-bill-alt"></i>';
                                                    break;
                                            }

                                            typeOfSelectedActivity.innerHTML = newIcon + " " + type;
                                        
                                            descriptionOfSelectedActivity.textContent = jsonData.Years[j].Campaigns[k].Campaign[l].Activities[a].Description;

                                            for (var z = 0; z < monthlyButtons.length; z++) {
                                                if (monthlyButtons[z].textContent == this.textContent) {
                                                    $(monthlyButtons[z]).addClass("active");
                                                } else {
                                                    $(monthlyButtons[z]).removeClass("active");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

    // ******************
    // Campaign Monthly events
    // CAPITAL D COLON
    for (var i = 0; i < monthlyButtons.length; i++) {
        monthlyButtons[i].addEventListener("click", function () {
            for (var j = 0; j < jsonData.Years.length; j++) {
                if (j == currentYearIndex) {
                    for (var k = 0; k < jsonData.Years[j].Campaigns.length; k++) {
                        if (k == currentCampaignIndex) {
                            for (var l = 0; l < jsonData.Years[j].Campaigns[k].Campaign.length; l++) {
                                if (jsonData.Years[j].Campaigns[k].Campaign[l].Name == this.textContent) {
                                    titleOfSelectedActivity.textContent = jsonData.Years[j].Campaigns[k].Campaign[l].Name;

                                    typeOfSelectedActivity.style.display = "none";
                                    
                                    resetActivityClasses();

                                    descriptionOfSelectedActivity.textContent = jsonData.Years[j].Campaigns[k].Campaign[l].Description;
                                    
                                    for (var z = 0; z < monthlyButtons.length; z++) {
                                        if (monthlyButtons[z].textContent == this.textContent) {
                                            $(monthlyButtons[z]).addClass("active");
                                        } else {
                                            $(monthlyButtons[z]).removeClass("active");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    function resetActivityClasses() {
        for (var m = 0; m < buttonsForActivities.length; m++) {
            if ($(buttonsForActivities[m]).hasClass("activeGreenActivity")) {
                $(buttonsForActivities[m]).removeClass("activeGreenActivity");
            } else if ($(buttonsForActivities[m]).hasClass("activeBlueActivity")) {
                $(buttonsForActivities[m]).removeClass("activeBlueActivity");
            } else if ($(buttonsForActivities[m]).hasClass("activeOrangeActivity")) {
                $(buttonsForActivities[m]).removeClass("activeOrangeActivity");
            }
        }
    }

    // Setting up the defaults for the campaign monthly event
    titleOfSelectedActivity.textContent = jsonData.Years[currentYearIndex].Campaigns[currentCampaignIndex].Campaign[0].Name;

    typeOfSelectedActivity.style.display = "none";
    
    descriptionOfSelectedActivity.textContent = jsonData.Years[currentYearIndex].Campaigns[currentCampaignIndex].Campaign[0].Description;
});