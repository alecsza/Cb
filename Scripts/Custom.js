//$(document).ready(function () {
    // This WILL work because we are listening on the 'document', 
    // for a click on an element with an ID of #test-element
    $(document).on("click", ".btnModal", function () {
       
        var url = $(this).attr('data-url');
        $('.modal-content').load(url);

        $('#MModal').modal('show');
    });



//$('#loader').modal({
//    backdrop: 'static',
//    keyboard: false
//});

$(document).on("click", ".cr", function () {

    var check = $(this).parent().children('input');
    var partialUrl = $(this).attr("data-url");
    var urlP = "";
    //alert(check.is(":checked"));
    if (check.is(":checked")) {
        urlP = partialUrl + "&stare=1";
    }
    else {
        urlP = partialUrl + "&stare=2";
    }
        // it is checked

        $.ajax({
            type: "GET",
            traditional: true,
            async: true,
            cache: false,
            url: urlP,
            success: function (result) {
                $.notify(result.mesaj, "success");
            },
            error: function (result) {
                $.notify(result.mesaj, "error");
            }
        });
    

});

$(document).on("click", ".chartAction", function () {
    
    var url_data_chart = $(this).attr("data-url");
   // $('#ChartDiv').load(url_data_chart); 

    $('#myAreaChart').remove(); // this is my <canvas> element
    $('.chart-area').append('<canvas id="myAreaChart"><canvas>');

    var vectOx;
    var valori;
    var denumireChart;
  
    $.ajax({
        type: "GET",
        traditional: true,
        async: true,
        cache: false,
        url: url_data_chart,
        success: function (result) {
            vectOx = result.vOx;
            valori = result.vValori;
            denumireChart = result.denActiune;
            //alert(result.Vect1);
            var canvas = document.getElementById("myAreaChart");
            //const context = ctx.getContext('2d');
            //context.clearRect(0, 0, ctx.width, ctx.height);
           
            var myLineChart = new Chart(canvas, {
                type: 'line',
                data: {
                    //labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                    labels: vectOx,
                    datasets: [{
                        label: denumireChart,
                        lineTension: 0.3,
                        backgroundColor: "rgba(78, 115, 223, 0.05)",
                        borderColor: "rgba(78, 115, 223, 1)",
                        pointRadius: 3,
                        pointBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointBorderColor: "rgba(78, 115, 223, 1)",
                        pointHoverRadius: 3,
                        pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointHoverBorderColor: "rgba(78, 115, 223, 1)",
                        pointHitRadius: 10,
                        pointBorderWidth: 2,
                        data: valori,
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    layout: {
                        padding: {
                            left: 10,
                            right: 25,
                            top: 25,
                            bottom: 0
                        }
                    },
                    scales: {
                        xAxes: [{
                            time: {
                                unit: 'date'
                            },
                            gridLines: {
                                display: false,
                                drawBorder: false
                            },
                            ticks: {
                                maxTicksLimit: 7
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                maxTicksLimit: 5,
                                padding: 10,
                                // Include a dollar sign in the ticks
                                callback: function (value, index, values) {
                                    return number_format(value);
                                }
                            },
                            gridLines: {
                                color: "rgb(234, 236, 244)",
                                zeroLineColor: "rgb(234, 236, 244)",
                                drawBorder: false,
                                borderDash: [2],
                                zeroLineBorderDash: [2]
                            }
                        }],
                    },
                    legend: {
                        display: false
                    },
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        titleMarginBottom: 10,
                        titleFontColor: '#6e707e',
                        titleFontSize: 14,
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        intersect: false,
                        mode: 'index',
                        caretPadding: 10,
                        callbacks: {
                            label: function (tooltipItem, chart) {
                                var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                                return datasetLabel + number_format(tooltipItem.yLabel);
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr) {
            //debugger;  
            console.log(xhr.responseText);
            alert("Error has occurred..");
        }
    });
});

function clearCanvas() {

    var canvas = document.getElementById("myAreaChart");
    //const context = ctx.getContext('2d');

    //context.clearRect(0, 0, ctx.width, ctx.height);
    const context = canvas.getContext('2d');

    context.clearRect(0, 0, canvas.width, canvas.height);

}


//$(document).on("click", ".perioada", function () {

//    var url = $(this).attr('data-url');
//    window.location.href =url;
//   // $('.modal-content').load(url);

  
//});


//gen rutine admin

$(document).on("click", ".checkRA", function () {

    var check = $(this);
    var partialUrl = $(this).attr("data-url");
    var urlP = "";
    //alert(check.is(":checked"));
    if (check.is(":checked")) {
        urlP = partialUrl + "&stare=1";
    }
    else {
        urlP = partialUrl + "&stare=0";
    }
    // it is checked

    $.ajax({
        type: "GET",
        traditional: true,
        async: true,
        cache: false,
        url: urlP,
        success: function (result) {
            $.notify(result.mesaj, "success");
        },
        error: function (result) {
            $.notify(result.mesaj, "error");
        }
    });


});

$(document).on("change", "#creareGA", function () {

    var den = $(this).val();
    var encod_den = encodeURIComponent(den);
    var urlPartial = $(this).attr('data-url');
    var url = urlPartial + '?denumire=' + encod_den;

    $('#content').load(url);
   // window.open(url);
    // $('.modal-content').load(url);


});



//PAGINA PRINCIPALA 

$(document).on("click", ".mainContent", function () {

    var url = $(this).attr('data-url');
    $('#content').load(url);

});