const mediaQuery = window.matchMedia('(max-width: 1075px)');
// when the width gets smaller than #px

function changeLogo(e) {
    if (e.matches) { // If media query matches
        
    }
    else {
    }
}
mediaQuery.addListener(changeLogo);
changeLogo(mediaQuery);