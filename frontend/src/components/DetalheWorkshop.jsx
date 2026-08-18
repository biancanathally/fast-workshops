export function DetalheWorkshop({ ata, aoFechar }) {
  if (!ata) return null;

  return (
    <div className="modal-overlay" onClick={aoFechar}>
      <div className="modal-card" onClick={(e) => e.stopPropagation()}>
        <div className="top">
          <h3>{ata.workshop.nome}</h3>
          <button className="close" onClick={aoFechar} aria-label="Fechar">
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M18 6L6 18M6 6l12 12" />
            </svg>
          </button>
        </div>

        <p className="meta">
          {new Date(ata.workshop.dataRealizacao).toLocaleDateString('pt-BR')}
          &nbsp;·&nbsp;{ata.totalColaboradores} {ata.totalColaboradores === 1 ? 'presente' : 'presentes'}
        </p>

        <p className="attendee-label">Presentes</p>
        <div className="tags">
          {ata.colaboradores.map((c) => (
            <span key={c.id} className="tag">{c.nome}</span>
          ))}
        </div>
      </div>
    </div>
  );
}
