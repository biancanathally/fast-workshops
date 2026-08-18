export function DetalheWorkshop({ ata, aoFechar }) {
  if (!ata) return null;

  return (
    <div className="modal-overlay" onClick={aoFechar}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <button className="modal-fechar" onClick={aoFechar} aria-label="Fechar">×</button>

        <h2>{ata.workshop.nome}</h2>
        <p className="modal-data">
          {new Date(ata.workshop.dataRealizacao).toLocaleDateString('pt-BR')}
        </p>
        <p>{ata.workshop.descricao}</p>

        <h3>Colaboradores presentes ({ata.totalColaboradores})</h3>
        <ul>
          {ata.colaboradores.map((c) => (
            <li key={c.id}>{c.nome}</li>
          ))}
        </ul>
      </div>
    </div>
  );
}
