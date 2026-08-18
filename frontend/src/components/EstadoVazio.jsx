export function EstadoVazio({ tipo, mensagem }) {
  if (tipo === 'loading') return <p className="estado estado-loading">Carregando…</p>;
  if (tipo === 'erro') return <p className="estado estado-erro">{mensagem}</p>;
  return <p className="estado estado-vazio">Nenhuma ata encontrada com esses filtros.</p>;
}
