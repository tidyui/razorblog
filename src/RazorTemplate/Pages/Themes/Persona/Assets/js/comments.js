function getComments(id) {
    var xmlhttp = new XMLHttpRequest();

    xmlhttp.onreadystatechange = function() {
        if (xmlhttp.readyState == XMLHttpRequest.DONE) {
           if (xmlhttp.status == 200) {
               document.getElementById("comments").innerHTML = xmlhttp.responseText;
           }
        }
    };

    xmlhttp.open('GET', '/comments/' + id, true);
    xmlhttp.send();
}