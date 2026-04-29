import { apiRequest } from './client'
import type { LoginRequest, LoginResponse, RegisterRequest, UserDto } from './types'

export function login(payload: LoginRequest) {
  return apiRequest<LoginResponse>('/Auth/login', {
    method: 'POST',
    json: {
      email: payload.email,
      senha: payload.senha,
    },
  })
}

export function register(payload: RegisterRequest) {
  return apiRequest<UserDto>('/Users/register', {
    method: 'POST',
    json: {
      nomeCompleto: payload.nomeCompleto,
      idade: payload.idade,
      email: payload.email,
      senha: payload.senha,
    },
  })
}
