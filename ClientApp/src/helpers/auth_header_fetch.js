import { authHeader } from '.';

export async function fetchJson(url, history) {
    const requestOptions = {
      headers: authHeader()
    };
    const d = await fetch(url, requestOptions);
    if (!d.ok) {
      history.push('/login')
      return [false, ""] 
    }
    const json = await d.json();
    return [true, json];
}