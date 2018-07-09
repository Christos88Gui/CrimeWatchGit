function displayNextImage() {
    var image = document.getElementById('img1');
    if (document.getElementById('radioBtn1').checked == true) {
        document.getElementById('radioBtn2').checked = true;
        image.src = '../Content/Images/img2.png';
    }
    else if (document.getElementById('radioBtn2').checked == true) {
        document.getElementById('radioBtn3').checked = true;
        image.src = '../Content/Images/map.png';
    }
    else {
        document.getElementById('radioBtn1').checked = true;
        image.src = '../Content/Images/img1.png';
    }
}

function startTimer() {
    setRadioBtnHandlers();
    setInterval(displayNextImage, 7000);
    document.getElementById('radioBtn1').checked = true;
    document.getElementById('radioBtn4').checked = true;
    document.getElementById('radioBtn7').checked = true;
    document.getElementById('radioBtn10').checked = true;
}

function setRadioBtnHandlers() {
    $("#radioBtn1").click(function () { document.getElementById("img1").src = '../Content/Images/img1.png' });
    $("#radioBtn2").click(function () { document.getElementById("img1").src = '../Content/Images/img2.png' });
    $("#radioBtn3").click(function () { document.getElementById("img1").src = '../Content/Images/map.png' });

    $("#radioBtn4").click(function () { document.getElementById("img2").src = '../Content/Images/img1.png' });
    $("#radioBtn5").click(function () { document.getElementById("img2").src = '../Content/Images/img2.png' });
    $("#radioBtn6").click(function () { document.getElementById("img2").src = '../Content/Images/map_Blurry.png' });

    $("#radioBtn7").click(function () { document.getElementById("img3").src = '../Content/Images/map.png' });
    $("#radioBtn8").click(function () { document.getElementById("img3").src = '../Content/Images/logo.png' });
    $("#radioBtn9").click(function () { document.getElementById("img3").src = '../Content/Images/map_Blurry.png' });

    $("#radioBtn10").click(function () { document.getElementById("img4").src = '../Content/Images/map.png' });
    $("#radioBtn11").click(function () { document.getElementById("img4").src = '../Content/Images/logo.png' });
    $("#radioBtn12").click(function () { document.getElementById("img4").src = '../Content/Images/map_Blurry.png' });
}


$("#What").click(function () {
    $('html, body').animate({
        scrollTop: $("#one").offset().top - 200
    }, 1000);
});

$("#How").click(function () {
    $('html, body').animate({
        scrollTop: $("#two").offset().top - 200
    }, 1000);
});

$("#Contact").click(function () {
    $('html, body').animate({
        scrollTop: $("#footer").offset().top
    }, 1000);
});

$(".backToTopBtn").click(function () {
    $('html, body').animate({
        scrollTop: 0
    }, 1000);
});

$('.dropdown-toggle').click(function () {
    $(this).next('.dropdown-menu').slideToggle(300);
});