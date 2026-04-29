import { clearToken, getToken } from './authStorage'

type JsonBody = Record<string, unknown>

function normalizePath(path: string) {
  if (path.startsWith('/api')) {
    return path
  }

  const p = path.startsWith('/') ? path : `/${path}`
  return `/api${p}`
}

export async function apiRequest<T>(
  path: string,
  init?: RequestInit & { json?: JsonBody }
): Promise<T> {
  const url = normalizePath(path)

  const { json, ...restInit } = init ?? {}
  const headers = new Headers(restInit.headers ?? {})

  if (!headers.has('Accept')) {
    headers.set('Accept', 'application/json')
  }

  const token = getToken()
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  let body = restInit.body
  if (json !== undefined) {
    headers.set('Content-Type', 'application/json')
    body = JSON.stringify(json)
  }

  const res = await fetch(url, {
    ...restInit,
    headers,
    body,
  })

  if (res.status === 204) {
    return undefined as T
  }

  const text = await res.text()
  const data = text ? (JSON.parse(text) as unknown) : null

  if (!res.ok) {
    let message = res.statusText
    if (
      typeof data === 'object' &&
      data !== null &&
      'error' in data &&
      typeof (data as { error?: string }).error === 'string'
    ) {
      message = (data as { error: string }).error
    }
    if ((res.status === 401 || res.status === 403) && getToken()) {
      clearToken()
    }
    throw new Error(message || 'Erro ao chamar API')
  }

  return data as T
}
