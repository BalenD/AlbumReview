﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumsReviewRESTApi.Models
{
    public class AlbumForUpdateDto
    {
        public string Name { get; set; }
        public DateTimeOffset Released { get; set; }
    }
}
