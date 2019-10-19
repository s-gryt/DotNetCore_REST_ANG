﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement.API.Dtos;
using TourManagement.API.Helpers;
using TourManagement.API.Services;

namespace TourManagement.API.Controllers
{
    [Route("api/tours")]
    public class ToursController : Controller
    {
        private readonly ITourManagementRepository _tourManagementRepository;

        public ToursController(ITourManagementRepository tourManagementRepository)
        {
            _tourManagementRepository = tourManagementRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTours()
        {
            var toursFromRepo = await _tourManagementRepository.GetTours();

            var tours = Mapper.Map<IEnumerable<Tour>>(toursFromRepo);
            return Ok(tours);
        }


//        [HttpGet("{tourId}", Name = "GetTour")]
        //        public async Task<IActionResult> GetTour(int tourId)
        //        {
        //            var tourFromRepo = await _tourManagementRepository.GetTour(tourId);
        //
        //            if (tourFromRepo == null)
        //            {
        //                return BadRequest();
        //            }
        //
        //            var tour = Mapper.Map<Tour>(tourFromRepo);
        //
        //            return Ok(tour);
        //        }

        [HttpGet("{tourId}", Name = "GetTour")]
        [RequestHeaderMatchesMediaType
            ("Accept", new string[] {"application/vnd.marvin.tour+json"})]
        public async Task<IActionResult> GetTour(int tourId)
        {
            return await GetSpecificTour<Tour>(tourId);
        }

        [HttpGet("{tourId}")]
        [RequestHeaderMatchesMediaType
            ("Accept", new string[] {"application/vnd.marvin.tourwithestimatedprofits+json"})]
        public async Task<IActionResult> GetTourWithEstimatedProfits(int tourId)
        {
            return await GetSpecificTour<TourWithEstimatedProfits>(tourId);
        }

        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-type",
            new string[] {"application/vnd.marvin.tourforcreation+json"})]
        public async Task<IActionResult> AddTour([FromBody] TourForCreation tour)
        {
            if (tour == null)
            {
                return BadRequest();
            }

            return await AddSpecificTourAction(tour);
        }

        [HttpGet("{tourId}")]
        [RequestHeaderMatchesMediaType
            ("Accept", new string[] {"application/vnd.marvin.tourwithestimatedprofits+json"})]
        public async Task<IActionResult> AddTourWithManager([FromBody] TourWIthManagerForCreation tour)
        {
            if (tour == null)
            {
                return BadRequest();
            }

            return await AddSpecificTourAction(tour);
        }

        private async Task<IActionResult> AddSpecificTourAction<T>(T tour) where T : class
        {
            var tourEntity = Mapper.Map<Entities.Tour>(tour);

            await _tourManagementRepository.AddTour(tourEntity);

            if (!await _tourManagementRepository.SaveAsync())
            {
                throw new Exception("Adding a tour failed on save");
            }

            var tourToReturn = Mapper.Map<Tour>(tourEntity);

            return CreatedAtRoute("GetTour",
                new {tourId = tourToReturn.TourId}, tourToReturn);
        }

        private async Task<IActionResult> GetSpecificTour<T>(int tourId) where T : class
        {
            var tourFromRepo = await _tourManagementRepository.GetTour(tourId);
            
            if (tourFromRepo == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<T>(tourFromRepo));
        }
    }
}