export function ListaAtas({ atas, aoClicarWorkshop }) {
  return (
    <div className="ata-list">
      {atas.map((ata) => (
        <article key={ata.id} className="ata-card">
          <span className="date">
            {new Date(ata.workshop.dataRealizacao).toLocaleDateString('pt-BR')}
          </span>

          <h3>
            <button className="workshop-link" onClick={() => aoClicarWorkshop(ata)}>
              {ata.workshop.nome}
            </button>
          </h3>

          <p>{ata.workshop.descricao}</p>

          <span className="presence">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M20 6L9 17l-5-5" />
            </svg>
            {ata.totalColaboradores} {ata.totalColaboradores === 1 ? 'presente' : 'presentes'}
          </span>
        </article>
      ))}
    </div>
  );
}
