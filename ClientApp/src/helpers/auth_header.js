export function authHeader() {
    // return authorization header with jwt token
    var user_from_storage = localStorage.getItem('user');
    if (user_from_storage) {
        let user = JSON.parse(user_from_storage);
        if (user && user.token) {
            return { 'Authorization': 'Bearer ' + user.token };
        } else {
            return {};
        }
    }
}