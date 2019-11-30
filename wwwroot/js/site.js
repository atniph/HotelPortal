function ValidateForm(from, to) {
    var first = new Date(document.forms["form"][from].value);
    var last = new Date(document.forms["form"][to].value);
    var today = new Date();
    today.setHours(0, 0, 0, 0);

    if (first >= last) {
        alert("First day must be before last day!");
        return false;
    }
    if (first < today) {
        alert("You can't reserve a room from the past");
        return false;
    }
}

function ValidateSearch() {
    var min = Number(document.forms["form"]["min"].value);
    var max = Number(document.forms["form"]["max"].value);

    if (!max) {
        return ValidateForm("from", "to");
    }

    if (min > max) {
        alert("Maximum price should be higher than minimum!");
        return false;
    }
    return ValidateForm("from", "to");
}

function ValidateReserve() {
    var succes = ValidateForm("FirstDay", "LastDay");
    if (succes == false) {

        return false;
    } else {

        alert("Succesful reserve");
        return true;
    } 
}