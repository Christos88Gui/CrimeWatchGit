function colorRows() {
    var rows = document.getElementsByTagName("tr");
    // loops through each row
    for (i = 1; i < rows.length; i++) {
        if (rows[i].cells[2].innerText.includes("1")) {
            rows[i].style.backgroundColor = "lightblue";
        }
        else if (rows[i].cells[2].innerText.includes("2")) {
            rows[i].style.backgroundColor = "lightgreen";
        }
        else if (rows[i].cells[2].innerText.includes("3")) {
            rows[i].style.backgroundColor = "yellow";
        }
        else if (rows[i].cells[2].innerText.includes("4")) {
            rows[i].style.backgroundColor = "orange";
        }
        else {
            rows[i].style.backgroundColor = "lightcoral";
        }
    }
}
