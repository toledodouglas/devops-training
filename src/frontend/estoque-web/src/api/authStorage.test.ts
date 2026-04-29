import { afterEach, describe, expect, it } from 'vitest'
import { clearToken, getToken, setToken } from './authStorage'

describe('authStorage', () => {
  afterEach(() => {
    localStorage.clear()
  })

  it('stores and reads token', () => {
    expect(getToken()).toBeNull()
    setToken('abc')
    expect(getToken()).toBe('abc')
    clearToken()
    expect(getToken()).toBeNull()
  })
})
