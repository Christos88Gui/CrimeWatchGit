function autocomplete() {
    var address = document.getElementById('addressSearch1');
    var autocomplete = new google.maps.places.Autocomplete(address);
    autocomplete.setComponentRestrictions(
        { 'country': ['uk'] });
}

function validateForm() {
    var address = document.getElementById("addressSearch1").value;
    if (address == "") {
        window.alert("Please type an address or postcode");
        return false;
    }
}