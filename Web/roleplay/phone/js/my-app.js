var app = new Framework7({
    // App root element
    root: '#app',
    // App Name
    name: 'My App',
    // App id
    id: 'com.myapp.test',
    // Enable swipe panel
    panel: {
        swipe: 'left',
    },
    // Add default routes
    routes: [
        {
            path: '/about/',
            url: 'about.html'
        },
        {
            path: '/contacts/',
            url: 'contacts.html'
        },
        {
            path: '/addcontact/',
            url: 'addcontact.html'
        }
    ]
    // ... other parameters
});
var mainView = app.views.create('.view-main');
app.statusbar.show();

var $$ = Dom7;

function AddNewContact() {
    var formData = app.form.convertToData('#my-form');
    alert(JSON.stringify(formData));
}

function updateClock() {
    var now = new Date(), // current date
        months = ['January', 'February', '...']; // you get the idea
    time = now.getHours() + ':' + now.getMinutes(), // again, you get the idea

        // a cleaner way than string concatenation
        date = [now.getDate(),
            months[now.getMonth()],
            now.getFullYear()].join(' ');

    // set the content of the element with the ID time to the formatted string
    document.getElementById('statustime').innerHTML = time;

    // call this function again in 1000ms
    setTimeout(updateClock, 10000);
}
updateClock(); // initial call