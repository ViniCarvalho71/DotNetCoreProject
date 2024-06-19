using GameStore.Data;
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

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) => {

            if(string.IsNullOrEmpty(newGame.Name)){
                return Results.BadRequest("Name is required");
            }
            Game game = new(){
                Name = newGame.Name,
                Genre = dbContext.Genres.Find(newGame.GenreId),
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDto gameDto = new(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(GetGameEndpointName, new { id=game.Id}, gameDto);
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
