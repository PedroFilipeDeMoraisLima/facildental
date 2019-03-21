$().ready(function () {
    $('.celular').mask('(00) 00000-0000');
    $('.telefone').mask('(00) 0000-0000');
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.data').mask('00/00/0000');
    $('.hora').mask('00:00:00');
    $('.data_hora').mask('00/00/0000 00:00:00');
    $('.cep').mask('00000-000');
})