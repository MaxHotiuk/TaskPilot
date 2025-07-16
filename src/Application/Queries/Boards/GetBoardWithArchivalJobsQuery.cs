using Application.Common.Dtos.Boards;
using MediatR;
using System;

namespace Application.Queries.Boards;

public record GetBoardWithArchivalJobsQuery(Guid BoardId) : IRequest<BoardDto?>;
