import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function RegisterPage() {
  const { register, loading } = useAuth()
  const navigate = useNavigate()

  const [nomeCompleto, setNomeCompleto] = useState('')
  const [idade, setIdade] = useState(18)
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [feedback, setFeedback] = useState<string | null>(null)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setFeedback(null)

    try {
      await register({ nomeCompleto, idade, email: email.trim(), senha })
      navigate('/login', { replace: true })
    } catch {
      setFeedback(
        'Não foi possível concluir o cadastro. Verifique dados e maioridade.'
      )
    }
  }

  return (
    <div className="page">
      <div className="card">
        <h1>Cadastro</h1>
        <form className="form" onSubmit={onSubmit}>
          <label htmlFor="nome">Nome completo</label>
          <input
            id="nome"
            required
            value={nomeCompleto}
            onChange={(e) => setNomeCompleto(e.target.value)}
          />

          <label htmlFor="idade">Idade</label>
          <input
            id="idade"
            type="number"
            min={0}
            max={120}
            required
            value={idade}
            onChange={(e) => setIdade(Number(e.target.value))}
          />

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
            {loading ? 'Salvando…' : 'Criar conta'}
          </button>
        </form>

        {feedback ? <p className="error">{feedback}</p> : null}

        <p className="muted">
          Já tem conta?{' '}
          <Link className="link" to="/login">
            Entrar
          </Link>
        </p>
      </div>
    </div>
  )
}
