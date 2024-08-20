﻿@model IEnumerable < SitiosWeb.Model.SoliciditudIncapacida >

    @{

        string layout = null;
        if(User.IsInRole("JEFATURA"))
    {
    layout = "~/Views/Shared/_LayoutJefatura.cshtml";
}
    else if (User.IsInRole("SUPERVISOR")) {
    layout = "~/Views/Shared/_LayoutSupervisor.cshtml";
}
Layout = layout;
}
@{
    var successMessage = TempData["SuccessMessage"] as string;
    var errorMessage = TempData["ErrorMessage"] as string;
}
< !DOCTYPE html >
    <html lang="es">
        <head>
            <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Aprobación de incapacidades</title>
                    <link rel="stylesheet" type="text/css" href="~/css/AprobacionInc.css">

                    </head>
                    <body>
                        <div class="container">
                            <header>
                                <h1>Aprobación de incapacidades</h1>
                            </header>
                            <div class="search-section">
                                <label for="identificacion">Buscar Funcionario por identificación</label>
                                <div class="search-bar">
                                    <input type="text" id="identificacion" placeholder="Identificación">
                                        <button>Buscar</button>
                                </div>
                            </div>
                            <div class="results-section">
                                @if (!string.IsNullOrEmpty(successMessage))
                                {
                                    <div class="alert alert-success">@successMessage</div>
                                }

                                @if (!string.IsNullOrEmpty(errorMessage))
                                {
                                    <div class="alert alert-danger">@errorMessage</div>
                                }
                                <h2>Seleccione el colaborador que desea gestionar</h2>
                                <table>
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>Identificación</th>
                                            <th>Cantidad Días Fuera</th>
                                            <th>Fecha Inicio</th>
                                            <th>Fecha Final</th>
                                            <th>Puesto Laboral</th>
                                            <th>Tipo de Incapacidad</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @if (Model != null && Model.Any())
                                        {
                                            foreach(var permisos in Model)
                                        {
                                            <tr>
                                                <td><input type="checkbox" class="select-row" data-id="@permisos.IdEmpleado"></td>
                                                <td>@permisos.IdEmpleado</td>
                                                <td>@permisos.DiasHorasFuera</td>
                                                <td>@permisos.FechaInicio</td>
                                                <td>@permisos.FechaFin</td>
                                                <td>@permisos.puestoLaboral</td>
                                                <td>@permisos.IdTipoPermiso</td>
                                            </tr>
                                        }
                    }
                                        else
                                        {
                                            <tr>
                                                <td colspan="7">No se encontraron registros.</td>
                                            </tr>
                                        }
                                    </tbody>
                                </table>
                                <div class="action-buttons">
                                    <button class="accept-btn">Aceptar</button>
                                    <button class="cancel-btn">Denegar</button>
                                </div>
                            </div>
                        </div>
                        <script src="~/js/AprovacionODen.js"></script>
                    </body>
                </html>