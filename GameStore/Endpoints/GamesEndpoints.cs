using GameStore.Dtos;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games = [
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

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){

        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games/
        group.MapGet("/", () => games);

        // GET /games/1

        group.MapGet("/{id}", (int id) => {

            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);


        }).WithName(GetGameEndpointName);

        // POST /games

        group.MapPost("/", (CreateGameDto newGame) => {

            if(string.IsNullOrEmpty(newGame.Name)){
                return Results.BadRequest("Name is required");
            }
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

        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) => {
            var index = games.FindIndex(game => game.Id == id);

            if(index == -1){
                return Results.NotFound();
            }
            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id) => {

            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();
        });

        return group;
    }

}
