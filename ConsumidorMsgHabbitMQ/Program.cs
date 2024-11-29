using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        /* Console.WriteLine("Digite 'run' para executar");
         string msg = Console.ReadLine();*/

        var msgRetorno = "";
        //var body = Array.Empty<byte>();

            //nó da conexão no rabbiMQ
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

        using var connection = await factory.CreateConnectionAsync();
        using var canal = await connection.CreateChannelAsync();


        await canal.QueueDeclareAsync(
                    queue: "MensagemEnviada_1", //nome da fila
                    durable: false, //se o servidor for reiniciado a msg continua na fila
                    exclusive: false, //so pode ser acessada via conexão atual
                    autoDelete: true,//deletada quando o consumidor usa na fila
                    arguments: null
                );

        //solicita entrega de forma assincrona
        var consumerObj = new AsyncEventingBasicConsumer(canal);

        consumerObj.ReceivedAsync +=  (model, m) =>
        {
            var body = m.Body.ToArray();
            msgRetorno = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {msgRetorno}");
            return Task.CompletedTask;
        };

        await canal.BasicConsumeAsync(
                            queue: "MensagemEnviada_1",
                            autoAck: true,
                            consumer: consumerObj
                        );
              
        Console.WriteLine($" Mensagem recebida: {msgRetorno}");
        Console.ReadLine();

    }
}