// we are hooking up a button event handler in the window.onload event
window.onload = (e) => {
    document.querySelector("#search").onclick = searchButtonClicked
};

// this will store what the user searched for, we need it to be in script scope so 
// that we can access it from outside of our searchButtonClicked() function
let displayTerm = "";

// searchButtonClicked() will be called when the button is clicked.
function searchButtonClicked() {
    console.log("searchButtonClicked() called");

    // this URL is the Giphy Search endpoint 
    // An example of another endpoint, the Giphy "Trending" endpoint: https://api.giphy.com/v1/gifs/trending
    const GIPHY_URL = "https://api.giphy.com/v1/gifs/search?";

    // this API key identifies you to the owner of the service. 
    // API keys are used to track and control how the API is being used - so if the user of this 
    // key is abusing the service it can be "turned off".
    let myKey = "rJtB84B44s4fEY51FrrcNkeSMoVPCNrc";

    // build up our URL string
    // we specify a parameter - api_key - and then give it a value
    let url = GIPHY_URL;
    url += "api_key=" + myKey;

    // parse the user entered term we wish to search
    // by getting the .value of the text input field
    let term = document.querySelector("#searchterm").value;
    displayTerm = term;

    // get rid of leading and trailing spaces. URLs do not work with spaces!
    term = term.trim();

    // encodeURIComponent() will escape characters like spaces 
    // (because we still might have spaces in the middle of the search term), 
    // ampersands, $ signs, + symbols and so on so that they are properly represented for a URL. 
    // For example, a space becomes %20
    term = encodeURIComponent(term);

    // bail out if we have nothing to search for
    if (term.length < 1) return;

    // add the search term to the url - the web service requires this parameter name to be "q"
    // Note that parameters are formated as name=value and separated by ampersands.
    url += "&q=" + term;

    // grab the user chosen search limit from <select> and append it to the URL
    let limit = document.querySelector("#limit").value;
    url += "&limit=" + limit;

    // update the UI with the user's search term
    document.querySelector("#status").innerHTML = "<b>Searching for '" + displayTerm + "'</b>";

    // see what the url looks like in the console
    console.log(url);

    // Request data!
    getData(url);
}

// To download the data, we are going to utilize the XMLHTTPRequest API (also known as XHR)
// "Use XMLHttpRequest (XHR) objects to interact with servers. 
// You can retrieve data from a URL without having to do a full page refresh. 
// This enables a Web page to update just part of a page without disrupting what the user is doing. 
// XMLHttpRequest is used heavily in AJAX programming."

function getData(url) {
    // Create a new XHR object
    let xhr = new XMLHttpRequest();

    //set the onload handler
    xhr.onload = dataLoaded;

    // set the onerror handler
    xhr.onerror = dataError;

    // open connection and send the request
    // "GET" is the HTTP method we are using - it means that we are sending web service 
    // parameters in the query string (part of the URL) and not as a separate file 
    // (which is what the "PUT" method is)
    xhr.open("GET", url);
    xhr.send();
}

// Callback functions
function dataLoaded(e) {
    // after the data has loaded, get a reference back to the XHR object
    // event.target is the xhr object
    let xhr = e.target;

    // xhr.responseText is the JSON file we just downloaded
    console.log(xhr.responseText);

    // turn the text into a parsable JavaScript object
    let obj = JSON.parse(xhr.responseText);

    // if there are no results, bail out by returning
    if (!obj.data || obj.data.length == 0) {
        document.querySelector("#status").innerHTML = "<b>No results found for '" + displayTerm + "'</b>";
        return;
    }

    //  start building the HTML string
    let results = obj.data;
    console.log("results.length = " + results.length);
    let bigString = "";

    // loop though the array of results
    for (let i = 0; i < results.length; i++) {
        let result = results[i];

        // get the URL to the GIF - we will use this in an <img> tag
        let smallURL = result.images.fixed_width_downsampled.url;
        if (!smallURL) smallURL = "images/no-image-found.png";

        // grab the main Giphy page URL, which we will later put in a link and rating
        let url = result.url;
        let rating = (result.rating ? result.rating : "NA").toUpperCase();

        // build a <div> for each result - here we are using ES6 template string
        // **The ` character is a backtick, not a single quote
        // You can find this character on the upper-left of your keyboard under the tilde
        let line = `<div class='result'>`
        line += `<span><a target='_blank' href='${url}'>View on Giphy</a><p>Rating: ${rating}</p></span>`;
        line += `<img src='${smallURL}' title= '${result.id}' />`;
        line += `</div>`;

        // The old-fashioned way to build a string utilizing concatenation
        // Does the same thing as above
        // var line = "<div class = 'result'>";
        //     line += "<img src='";
        //     line += smallURL;
        //     line += "' title='";
        //     line += result.id;
        //     line += "' />";

        //     line += "<span><a target='_blank' href = '" + url + "'>View on Giphy</a></span>";
        //     line += "</div>";

        // add the <div> to bigString and loop
        bigString += line;
    }

    // all done building the HTML - show it to the user by updating the #content <div>
    document.querySelector("#content").innerHTML = bigString;

    // update the #status <div>
    document.querySelector("#status").innerHTML = "<b>Success!</b><p><i>Here are " + results.length + " results for '" + displayTerm + "'</i></p>";
}

function dataError(e) {
    console.log("An error just occurred!");
}