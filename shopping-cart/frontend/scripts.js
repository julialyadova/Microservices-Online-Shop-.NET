const baseURL = "/gateway/shopping-cart"

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

function getCart() {
    return new Promise((resolve, reject) => {
        resolve({
            totalPrice: 10000,
            items: [
                {
                    id: 1,
                    count: 5,
                    imageURL: 'https://img-global.cpcdn.com/recipes/4eef943beda41e49/1200x630cq70/photo.jpg',
                    name: 'Окунь в отрубях',
                    price: 999
                },
                {
                    id: 2,
                    count: 13,
                    imageURL: 'https://e2.edimdoma.ru/data/recipes/0003/5786/35786-ed4_wide.jpg?1468629935',
                    name: 'Килька в апельсиновом соусе',
                    price: 999
                },
                {
                    id: 3,
                    count: 1,
                    imageURL: 'https://sc04.alicdn.com/kf/U29e2ccfac7634f4cb088a74215d5f001H.png',
                    name: 'Анчоус сушеный',
                    price: 999
                }
            ]
        });
        // return;
        // sendRequest({}, baseURL, 'get').then((xhr) => {
        //     resolve(JSON.parse(xhr.response));
        // }).catch(reject);
    });
}
// deleteAll - если true удалить все товары одного типа, false - только один товар
function deleteCartItem(id, deleteAll = false) {
    return new Promise((resolve, reject) => {
        sendRequest({}, `${baseURL}?id=${id}&deleteAll=${deleteAll}`, 'delete').then((xhr) => {
            resolve(JSON.parse(xhr.response));
        }).catch(reject);
    });
}

function addCartItem(id) {
    return new Promise((resolve, reject) => {
        sendRequest({}, `${baseURL}?id=${id}`, 'post').then((xhr) => {
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