

function DrawChart(chartMonths, NumberOfTicketsBoughtLastyear, GrossTicketSalesLastyear) {
    var lineChartData = {
        labels: chartMonths,
        datasets: [
        {
            fillColor: "rgba(220,220,220,0.2)",
            strokeColor: "rgba(220,220,220,1)",
            pointColor: "rgba(220,220,220,1)",
            pointStrokeColor: "#fff",
            pointHighlightFill: "#fff",
            pointHighlightStroke: "rgba(220,220,220,1)",
            data: NumberOfTicketsBoughtLastyear
        }
        ]
    }
    var lineChartData2 = {
        labels: chartMonths,
        datasets: [
                {
                    fillColor: "rgba(151,187,205,0.2)",
                    strokeColor: "rgba(151,187,205,1)",
                    pointColor: "rgba(151,187,205,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(151,187,205,1)",
                    data: GrossTicketSalesLastyear
                }
        ]
    }
    var ctx = document.getElementById('myChart').getContext("2d");
    var ctx2 = document.getElementById('myChart2').getContext("2d");
    var myLine = new Chart(ctx).Line(lineChartData, { responsive: true });
    var myLine2 = new Chart(ctx2).Line(lineChartData2, { responsive: true });
}

