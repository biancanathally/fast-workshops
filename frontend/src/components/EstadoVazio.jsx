export function EstadoVazio({ tipo, mensagem }) {
  const texto =
    tipo === 'loading' ? 'Carregando…'
    : tipo === 'erro' ? mensagem
    : 'Nenhuma ata encontrada com esses filtros.';

  return <p className="state-note">{texto}</p>;
}
