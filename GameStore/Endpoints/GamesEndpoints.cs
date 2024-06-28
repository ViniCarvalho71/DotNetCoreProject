using GameStore.Data;
using GameStore.Dtos;
using GameStore.Entities;
using GameStore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){

        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games/
        group.MapGet("/", (GameStoreContext dbContext) => dbContext.Games.Include(game => game.Genre).Select(game => game.ToGameSummaryDto()).AsNoTracking());

        // GET /games/1

        group.MapGet("/{id}", (int id, GameStoreContext dbContext) => {

            Game? game = dbContext.Games.Find(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());


        }).WithName(GetGameEndpointName);

        // POST /games

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) => {

            if(string.IsNullOrEmpty(newGame.Name)){
                return Results.BadRequest("Name is required");
            }

            Game game = newGame.ToEntity();
           
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id=game.Id}, game.ToGameDetailsDto());
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => {
            var existingGame = dbContext.Games.Find(id);

            if(existingGame is null){
                return Results.NotFound();
            }
            
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));

            dbContext.SaveChanges();

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) => {

            dbContext.Games.Where(game => game.Id == id).ExecuteDelete();

            return Results.NoContent();
        });

        return group;
    }

}
