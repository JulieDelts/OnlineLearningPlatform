﻿namespace OnlineLearningPlatform.BLL.Exceptions;

public class EntityConflictException(string message) : Exception(message)
{ }
