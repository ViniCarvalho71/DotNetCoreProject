using GameStore.Dtos;
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

const string GetGameEndpointName = "GetGame";

List<GameDto> games = [
        new (1,
        "God of War 2018",
        "Adventure",
        20.0M,
        new DateOnly(2018,10,05)),
    new (2,
        "Red Dead Redemption II",
        "Adventure",
        25.0M,
        new DateOnly(2019,07,09))
];

// GET /games/
app.MapGet("games", () => games);

// GET /games/1

app.MapGet("games/{id}", (int id) => games.Find(game => game.Id == id)).WithName(GetGameEndpointName);

// POST /games

app.MapPost("games", (CreateGameDto newGame) => {
    GameDto game = new (
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );

    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id=game.Id}, game);
});


app.Run();
