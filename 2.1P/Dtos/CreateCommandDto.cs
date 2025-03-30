using System;

namespace _2._1P.Dtos;

public record class CreateCommandDto(string CommandName, bool IsMove);