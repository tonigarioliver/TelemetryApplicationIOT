window.showalert = (message) => {
    alert(message)
}

window.createplot = (chartid, lineNames) => {
    let time = new Date();

    // Initialize initialData here
    let initialData = lineNames.map(() => 0.0);

    let data = lineNames.map((lineName, index) => ({
        x: [time],
        y: [initialData[index]],
        mode: "lines",
        name: lineName,
        line: {
            color: getLineColor(index),
        }
    }));

    Plotly.newPlot(chartid, data);
}


window.extendplot = (chartid, lineIndex, payload) => {
    let time = new Date();
    let update = {
        x: [[time]],
        y: [[payload]]
    };
    let oldTime = time.setMinutes(time.getMinutes() - 1);
    let futureTime = time.setMinutes(time.getMinutes() + 1);
    let minuteView = {
        xaxis: {
            type: "date",
            range: [oldTime, futureTime]
        }
    };
    Plotly.relayout(chartid, minuteView);
    Plotly.extendTraces(chartid, update, [lineIndex]);
}

function getLineColor(index) {
    // Define una función que asigna un color basado en el índice de la línea
    // Puedes personalizar esta función para que devuelva colores específicos
    const colors = ["#8B0000", "#00FF00", "#0000FF", "#FF00FF"];
    return colors[index % colors.length];
}

