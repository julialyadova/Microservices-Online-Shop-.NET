const baseURL = "/gateway/admin"

function sendRequest(dataObject, url = '/item', method="post") {
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

function getItems() {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + '/item/list', 'get').then((xhr) => {
            resolve(Array.from(JSON.parse(xhr.response)));
        }).catch(reject);
    });
}

function getItemsByCategory(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + '/item/list?category='+id, 'get').then((xhr) => {
            resolve(Array.from(JSON.parse(xhr.response)));
        }).catch(reject);
    });
}

function getItem(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + `/item/${id}`, 'get').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function addItem(item) {
    return new Promise((resolve, reject) => {
        sendRequest(item, baseURL + '/item', 'post').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function updateItem(item) {
    return new Promise((resolve, reject) => {
        sendRequest(item, baseURL + `/item`, 'put').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function deleteItem(id) {
    return new Promise((resolve, reject) => {
        sendRequest(id, baseURL + `/item/${id}`, 'delete').then((xhr) => {
            resolve(xhr.response);
        }).catch(reject);
    })
}

function getCategories() {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + '/category/all', 'get').then((xhr) => {
            resolve(Array.from(JSON.parse(xhr.response)));
        }).catch(reject);
    });
}

function getCategoriesByParent(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + '/category/list?parent=' + id, 'get').then((xhr) => {
            resolve(Array.from(JSON.parse(xhr.response)));
        }).catch(reject);
    });
}

function getCategory(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, baseURL + `/category/${id}`, 'get').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function addCategory(category) {
    return new Promise((resolve, reject) => {
        sendRequest(category, baseURL + '/category', 'post').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function updateCategory(category) {
    return new Promise((resolve, reject) => {
        sendRequest(category, baseURL + '/category', 'put').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function deleteCategory(id) {
    return new Promise((resolve, reject) => {
        sendRequest(id, baseURL + `/category/${id}`, 'delete').then((xhr) => {
            resolve(xhr.response);
        }).catch(reject);
    })
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


let ItemsList = (() => {

    let that = {};
    that.app = null;

    that.init = () => {
      that.app = new Vue({
        el: '#app',
        data: {
          items: []
        }
      });


      getItems().then((val) => {
        that.app.items = val;
      });
    }

    return {
      init: that.init
    };
  })();