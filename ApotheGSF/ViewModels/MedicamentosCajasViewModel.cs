﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApotheGSF.ViewModels
{
    public class MedicamentosCajasViewModel
    {
        [Display(Name = "Código: ")]
        public int CodigoCaja { get; set; }
        [Display(Name = "Código del Medicamento: ")]
        public int CodigoMedicamento { get; set; }
        [Display(Name = "Código del Laboratorio: ")]
        public int CodigoLaboratorio { get; set; }
        [Required(ErrorMessage = "Digite una cantidad")]
        [Display(Name = "Cantidad de Unidades: ")]
        public int CantidadUnidad { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Costo: ")]
        public float Costo { get; set; }
        [Required(ErrorMessage = "Este campo no puede estar vacio")]
        [Display(Name = "Precio por Unidad: ")]
        public float PrecioUnidad { get; set; }
        [Required(ErrorMessage = "Digite la fecha de adquisición")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Fecha de Adquisición: ")]
        public DateTime FechaAdquirido { get; set; }
        [Required(ErrorMessage = "Digite la fecha de vencimiento")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [Display(Name = "Fecha de Vencimiento: ")]
        public DateTime FechaVencimiento { get; set; }
        [Display(Name = "Detallada? ")]
        public bool Detallada { get; set; }
        public bool Inactivo { get; set; }
        [Display(Name = "Medicamento: ")]
        public string? NombreMedicamento { get; set; }
        [Required(ErrorMessage = "Digite una cantidad")]
        [Display(Name = "Lotes: ")]
        public int Cajas { get; set; }

        [Display(Name = "Fecha de Creación: ")]

        public DateTime? Creado { get; set; }
        [Display(Name = "Creado por: ")]
        public string? CreadoNombreUsuario { get; set; }
        [Display(Name = "Última Modificación: ")]
        public DateTime? Modificado { get; set; }
        [Display(Name = "Modificado por: ")]
        public string? ModificadoNombreUsuario { get; set; }
    }
}
