import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function LoginPage() {
  const { login, loading } = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [feedback, setFeedback] = useState<string | null>(null)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setFeedback(null)
    try {
      await login(email, senha)
      navigate('/products', { replace: true })
    } catch {
      setFeedback('Não foi possível entrar. Verifique e-mail e senha.')
    }
  }

  return (
    <div className="page">
      <div className="card">
        <h1>Entrar</h1>
        <form className="form" onSubmit={onSubmit}>
          <label htmlFor="email">E-mail</label>
          <input
            id="email"
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <label htmlFor="senha">Senha</label>
          <input
            id="senha"
            type="password"
            required
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
          />

          <button disabled={loading} type="submit" className="primary">
            {loading ? 'Entrando…' : 'Entrar'}
          </button>
        </form>

        {feedback ? <p className="error">{feedback}</p> : null}

        <p className="muted">
          Não tem conta?{' '}
          <Link className="link" to="/register">
            Cadastre-se
          </Link>
        </p>
      </div>
    </div>
  )
}
