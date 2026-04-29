const key = 'estoque_token'

export function getToken(): string | null {
  return localStorage.getItem(key)
}

export function setToken(token: string) {
  localStorage.setItem(key, token)
}

export function clearToken() {
  localStorage.removeItem(key)
}
