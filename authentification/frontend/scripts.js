const baseURL = "/gateway/" // или authentification/api/

function sendRequest(dataObject, url, method="get") {
    return new Promise((resolve, reject) => {
        let xhr = new XMLHttpRequest();
            xhr.open(method, url, true);
            xhr.setRequestHeader("Content-Type", 'application/json');

        xhr.onreadystatechange = () => {
            if(xhr.readyState !== 4) return ;
            return (xhr.status === 200 || xhr.status === 202) ? resolve(xhr) : reject(xhr);
        }

        xhr.send(JSON.stringify(dataObject));
    });
}


function Register(registerModel) {
    return new Promise((resolve, reject) => {
        sendRequest(registerModel, baseURL + 'register', 'post').then((xhr) => {
            let token = xhr.getResponseHeader("Authorization").split(" ")[1];
            setCookie("token", token)
            resolve(token);
        }).catch(reject);
    });
}

function Login(loginModel) {
    return new Promise((resolve, reject) => {
        sendRequest(loginModel, baseURL + 'login', 'post').then((xhr) => {
            let token = xhr.getResponseHeader("Authorization").split(" ")[1];
            setCookie("token", token)
            resolve(token);
        }).catch(reject);
    });
}

function parseJwt(token) {
    return atob(token.split('.')[1]);
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
