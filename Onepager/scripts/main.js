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