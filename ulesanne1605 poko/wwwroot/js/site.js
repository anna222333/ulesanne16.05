// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FillCities(lstCountryCtrl, lstCityId) {
    var lstCities = $("#" + lstCityId);
    lstCities.empty();

    lstCities.append($('<option/>', {
        value: null,
        text: 'Select City'
    }));

    var selectedCountry = lstCountryCtrl.options[lstCountryCtrl.selectedIndex].value;
    if (selectedCountry !== null && selectedCountry !== '') {
        $.getJSON('/Customer/getcitiesbycountry', { countryId: selectedCountry }, function (cities) {
            if (cities !== null && !jQuery.isEmptyObject(cities)) {
                $.each(cities, function (index, city) {
                    lstCities.append($('<option/>', {
                        value: city.value,
                        text: city.text
                    }));
                });
            }
        });
    }
    return;
}

$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop(); 
    document.getElementById('PreviewPhoto').src = window.URL.createObjectURL(this.files[0]);
    document.getElementById('PhotoUrl').value = fileName;
});


document.getElementById('addCountryForm').addEventListener('submit', function (e) {
    e.preventDefault();
    var name = document.getElementById('newCountryName').value;
    fetch('/Country/CreateFromModal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: name })
    })
        .then(response => response.json())
        .then(data => {
            if (data && data.id) {
               
                var select = document.querySelector('select[name="CountryId"]');
                var option = new Option(data.name, data.id, true, true);
                select.add(option);
              
                var modal = bootstrap.Modal.getInstance(document.getElementById('addCountryModal'));
                modal.hide();
            }
        });
});


document.getElementById('addCityForm').addEventListener('submit', function (e) {
    e.preventDefault();
    var name = document.getElementById('newCityName').value;
    var countryId = document.getElementById('newCityCountryId').value;
    fetch('/City/CreateFromModal', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: name, CountryId: countryId })
    })
        .then(response => response.json())
        .then(data => {
            if (data && data.id) {
               
                var select = document.querySelector('select[name="CityId"]');
                var option = new Option(data.name, data.id, true, true);
                select.add(option);
                
                var modal = bootstrap.Modal.getInstance(document.getElementById('addCityModal'));
                modal.hide();
            }
        });
});
