const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
function clockTicker() {
    var date = new Date();
    var day = date.getDay();
    var hrs = date.getHours();
    var mins = date.getMinutes();
    var secs = date.getSeconds();
    if (hrs > 12) {

        hrs = hrs - 12;

        document.querySelector('#clock .period').innerHTML = 'PM';
    }
    else {

        document.querySelector('#clock .period').innerHTML = 'AM';
    }

    hrs = hrs < 10 ? "0" + hrs : hrs;
    mins = mins < 10 ? "0" + mins : mins;
    secs = secs < 10 ? "0" + secs : secs;


    document.querySelector('#clock .day').innerHTML = weekDays[day];
    document.querySelector('#clock .hours').innerHTML = hrs;
    document.querySelector('#clock .minutes').innerHTML = mins;
    document.querySelector('#clock .seconds').innerHTML = secs;

  
    requestAnimationFrame(clockTicker);
}


function showtoast(m, t) {
    toastr.clear();
    NioApp.Toast(m, t, { timeOut: 5000, position: "top-right"});
}