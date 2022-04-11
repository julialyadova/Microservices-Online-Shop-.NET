const baseURL = "/gateway/catalog"

function sendRequest(dataObject, url, method="get") {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
            xhr.open(method, url, true);
            xhr.setRequestHeader("Content-Type", 'application/json');
            xhr.setRequestHeader("Authorization", `Bearer ${getCookie("token")}`);

        xhr.onreadystatechange = () => {
            if(xhr.readyState !== 4) return ;
            return (xhr.status === 200 || xhr.status === 202) ? resolve(xhr) : reject(xhr);
        }

        xhr.send(JSON.stringify(dataObject));
    });
}


function getItemsByCategory(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + '?category='+id, 'get').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function getParam(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function setCookie(name,value) {
    document.cookie = name + "=" + (value || "")  + "; path=/";
}
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
        var c = ca[i];
        while (c.charAt(0)==' ') c = c.substring(1,c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
    }
    return null;
}
