using cxserver.Application.Features.MenuEngine.Commands.MenuGroups;
using cxserver.Application.Features.MenuEngine.Commands.MenuItems;
using cxserver.Application.Features.MenuEngine.Commands.Menus;
using cxserver.Application.Features.MenuEngine.Queries.GetMenuGroups;
using cxserver.Application.Features.MenuEngine.Queries.GetMenuItemTree;
using cxserver.Application.Features.MenuEngine.Queries.GetMenusByGroup;
using cxserver.Application.Features.MenuEngine.Queries.GetRenderMenus;
using cxserver.Domain.MenuEngine;
using MediatR;

namespace cxserver.Endpoints;

public static class MenuManagementEndpoints
{
    public static RouteGroupBuilder MapMenuManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var admin = app.MapGroup("/api/admin");

        admin.MapGet("/menu-groups", async (bool includeGlobal, bool activeOnly, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetMenuGroupsQuery(includeGlobal, activeOnly), cancellationToken);
            return Results.Ok(response);
        });

        admin.MapPost("/menu-groups", async (CreateMenuGroupRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var created = await sender.Send(
                new CreateMenuGroupCommand(request.TenantId, request.Type, request.Name, request.Slug, request.Description, request.IsActive),
                cancellationToken);

            return Results.Created($"/api/admin/menu-groups/{created.Id}", created);
        });

        admin.MapPatch("/menu-groups/{id:guid}", async (Guid id, UpdateMenuGroupRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var updated = await sender.Send(
                new UpdateMenuGroupCommand(id, request.Name, request.Slug, request.Description, request.IsActive),
                cancellationToken);

            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        admin.MapDelete("/menu-groups/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var deleted = await sender.Send(new DeleteMenuGroupCommand(id), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        admin.MapGet("/menu-groups/{id:guid}/menus", async (Guid id, bool activeOnly, ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetMenusByGroupQuery(id, activeOnly), cancellationToken);
            return Results.Ok(response);
        });

        admin.MapPost("/menus", async (CreateMenuRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var created = await sender.Send(
                new CreateMenuCommand(
                    request.MenuGroupId,
                    request.TenantId,
                    request.Name,
                    request.Slug,
                    request.Variant,
                    request.IsMegaMenu,
                    request.Order,
                    request.IsActive),
                cancellationToken);

            return Results.Created($"/api/admin/menus/{created.Id}", created);
        });

        admin.MapPatch("/menus/{id:guid}", async (Guid id, UpdateMenuRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var updated = await sender.Send(
                new UpdateMenuCommand(id, request.Name, request.Slug, request.Variant, request.IsMegaMenu, request.Order, request.IsActive),
                cancellationToken);

            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        admin.MapDelete("/menus/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var deleted = await sender.Send(new DeleteMenuCommand(id), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        admin.MapGet("/menus/{id:guid}/items", async (Guid id, bool activeOnly, ISender sender, CancellationToken cancellationToken) =>
        {
            var tree = await sender.Send(new GetMenuItemTreeQuery(id, activeOnly), cancellationToken);
            return Results.Ok(tree);
        });

        admin.MapPost("/menu-items", async (CreateMenuItemRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var created = await sender.Send(
                new CreateMenuItemCommand(
                    request.MenuId,
                    request.TenantId,
                    request.ParentId,
                    request.Title,
                    request.Slug,
                    request.Url,
                    request.Target,
                    request.Icon,
                    request.Description,
                    request.Order,
                    request.IsActive),
                cancellationToken);

            return Results.Created($"/api/admin/menu-items/{created.Id}", created);
        });

        admin.MapPatch("/menu-items/{id:guid}", async (Guid id, UpdateMenuItemRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var updated = await sender.Send(
                new UpdateMenuItemCommand(
                    id,
                    request.ParentId,
                    request.Title,
                    request.Slug,
                    request.Url,
                    request.Target,
                    request.Icon,
                    request.Description,
                    request.Order,
                    request.IsActive),
                cancellationToken);

            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        admin.MapDelete("/menu-items/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var deleted = await sender.Send(new DeleteMenuItemCommand(id), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        admin.MapPatch("/menus/{id:guid}/items/reorder", async (Guid id, ReorderMenuItemsRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var updated = await sender.Send(new ReorderMenuItemsCommand(id, request.Items), cancellationToken);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        var web = app.MapGroup("/api/web");

        web.MapGet("/menus", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetRenderMenusQuery(false), cancellationToken);
            return Results.Ok(response);
        });

        return admin;
    }

    public sealed record CreateMenuGroupRequest(
        Guid? TenantId,
        MenuGroupType Type,
        string Name,
        string Slug,
        string? Description,
        bool IsActive);

    public sealed record UpdateMenuGroupRequest(
        string Name,
        string Slug,
        string? Description,
        bool IsActive);

    public sealed record CreateMenuRequest(
        Guid MenuGroupId,
        Guid? TenantId,
        string Name,
        string Slug,
        MenuVariant Variant,
        bool IsMegaMenu,
        int Order,
        bool IsActive);

    public sealed record UpdateMenuRequest(
        string Name,
        string Slug,
        MenuVariant Variant,
        bool IsMegaMenu,
        int Order,
        bool IsActive);

    public sealed record CreateMenuItemRequest(
        Guid MenuId,
        Guid? TenantId,
        Guid? ParentId,
        string Title,
        string Slug,
        string Url,
        MenuItemTarget Target,
        string? Icon,
        string? Description,
        int Order,
        bool IsActive);

    public sealed record UpdateMenuItemRequest(
        Guid? ParentId,
        string Title,
        string Slug,
        string Url,
        MenuItemTarget Target,
        string? Icon,
        string? Description,
        int Order,
        bool IsActive);

    public sealed record ReorderMenuItemsRequest(IReadOnlyList<ReorderMenuItemsEntry> Items);
}
