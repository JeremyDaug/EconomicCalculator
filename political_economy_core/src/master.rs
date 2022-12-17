//! Master, this is the root for our simulation program going forward.
//! 
//! # Data
//! 
//! - Core Data
//!   - Sets
//!   - Wants
//!   - Technologies
//!   - Technology Families
//!   - Products
//!   - Skills
//!   - Skill Groups
//!   - Processes
//!   - Jobs
//! - Recordkeeping
//!   - Species
//!     - Subtypes (Sex/Caste)
//!     - Cohorts (Life Phases)
//!   - Civilization (Not Built)
//!     - Epoch (Ages)
//!   - Culture
//!     - Class
//!     - Generation
//!   - Ideology (Not Built)
//!     - Wave
//!     - Faction
//!   - Movements (Not Built)
//! - Environmental
//!   - Territories
//!   - Markets
//! - Actors
//!   - Pops
//!   - Firms
//!   - Institutions (Not Built)
//!   - States (Not Built)
//! 
//! # Data Details
//! 
//! ## Core Data
//! 
//! Core Data is the data which should never need to change during regular play.
//! It only changes during the "Update" logical phase, which bookends our cycle
//! and should be available to access through immutable references passed 
//! around through the rest of the system.
//! 
//! ## Recordkeeping
//! 
//! Recordkeeping data are those things which are primarily reference data, but 
//! will be updated and record changes made during the appropriate change phase.
//! The changes will primarily take place during the population phase, when new
//! pops will be created, altered, assimilated, converted, or die.
//! 
//! The Recordkeepers are handled collectively in a single thread as they
//! shouldn't need additional aid and 
//! 
//! Hypothetically, it is available as an immutable reference elsewhere in the
//! code during most phases. During the Pop Change phase, it should be a
//! listener, rather than a reference. The Pop Threads making changes record
//! basic data, send that over to the Recordkeepers, and they send back
//! additional data, for the pop thread to address and modify further (if
//! necissary).
//! 
//! ## Environmental
//! 
//! Environmental data are the places where actors are interacting in directly.
//! These are in their own thread, with Markets being the primary, and territory 
//! (the map) being secondary and buffered by the market. Markets are the primary
//! interface between the actors and the map, the market has a compiled copy of
//! the territories' info that it contains.
//! 
//! The Map will try to do it's calculation work while the rest of the system works
//! allowing to offload stuff. The hope is that the map can be going over it's 
//! data, updating available resources, calculating climatalogical changes like
//! weather, tempurature, rain, etc. Hopefully this can be done with minimal
//! collisions with the Market. 
//! 
//! Map alterations may be split into 2 phases, an exchange phase between it and 
//! the market and a climate phase, getting the changes for the new map. Or
//! the market and map will work together to start, then once market work is done
//! the map will begun updating.
//! 
//! ## Actors
//! 
//! Actors are those who can act and have an AI attached to them. Pops, Firms,
//! Institutions, and States (states may be the same as Institutions), can
//! all act and interact.
//! 
//! Each actor is given it's own thread so it may act with the market it is
//! connected to, and may also communicate with it's various connections
//! such as parent or child firms which share a market.
//! 
//! Actors and their Market communicate by bus to collect data and try and 
//! exchange goods.

