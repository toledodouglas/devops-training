import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { apiRequest } from './client'

describe('apiRequest', () => {
  beforeEach(() => {
    vi.stubGlobal(
      'fetch',
      vi.fn(async () =>
        new Response(JSON.stringify({ accessToken: 't' }), {
          status: 200,
          headers: { 'Content-Type': 'application/json' },
        })
      ) as typeof fetch
    )
    localStorage.setItem('estoque_token', 'abc')
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
    localStorage.clear()
  })

  it('sends auth header when token exists', async () => {
    const spy = vi.spyOn(globalThis, 'fetch')

    await apiRequest<{ accessToken: string }>(`/Auth/login`, {
      method: 'POST',
      json: { email: 'a', senha: 'b' },
    })

    expect(spy).toHaveBeenCalled()
    const init = spy.mock.calls[0]?.[1] as RequestInit
    expect(init?.headers).toBeDefined()
    const headers = init?.headers instanceof Headers ? init.headers : new Headers()
    expect(headers.get('Authorization')).toBe('Bearer abc')
  })
})
