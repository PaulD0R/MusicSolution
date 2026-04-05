--
-- PostgreSQL database dump
--

\restrict hqho7OOwNcOAhdrtMuYzTX3EFLLGCqejFHsvZLRvOnDXCmlgyLCxlEEmBoEoiHv

-- Dumped from database version 16.13 (Ubuntu 16.13-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.13 (Ubuntu 16.13-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: pg_trgm; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS pg_trgm WITH SCHEMA public;


--
-- Name: EXTENSION pg_trgm; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION pg_trgm IS 'text similarity measurement and index searching based on trigrams';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Likes; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."Likes" (
    "MusicId" uuid NOT NULL,
    "UserId" text NOT NULL,
    "MusicDataId" uuid
);


ALTER TABLE public."Likes" OWNER TO pauldor;

--
-- Name: MusicData; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."MusicData" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Bitrate" integer NOT NULL,
    "Path" text NOT NULL
);


ALTER TABLE public."MusicData" OWNER TO pauldor;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: pauldor
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO pauldor;

--
-- Data for Name: Likes; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."Likes" ("MusicId", "UserId", "MusicDataId") FROM stdin;
\.


--
-- Data for Name: MusicData; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."MusicData" ("Id", "Name", "Bitrate", "Path") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: pauldor
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260404151228_InitialCreate	10.0.5
\.


--
-- Name: Likes PK_Likes; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Likes"
    ADD CONSTRAINT "PK_Likes" PRIMARY KEY ("MusicId", "UserId");


--
-- Name: MusicData PK_MusicData; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."MusicData"
    ADD CONSTRAINT "PK_MusicData" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Likes_MusicDataId; Type: INDEX; Schema: public; Owner: pauldor
--

CREATE INDEX "IX_Likes_MusicDataId" ON public."Likes" USING btree ("MusicDataId");


--
-- Name: IX_MusicData_Name; Type: INDEX; Schema: public; Owner: pauldor
--

CREATE INDEX "IX_MusicData_Name" ON public."MusicData" USING gin ("Name" public.gin_trgm_ops);


--
-- Name: Likes FK_Likes_MusicData_MusicDataId; Type: FK CONSTRAINT; Schema: public; Owner: pauldor
--

ALTER TABLE ONLY public."Likes"
    ADD CONSTRAINT "FK_Likes_MusicData_MusicDataId" FOREIGN KEY ("MusicDataId") REFERENCES public."MusicData"("Id");


--
-- PostgreSQL database dump complete
--

\unrestrict hqho7OOwNcOAhdrtMuYzTX3EFLLGCqejFHsvZLRvOnDXCmlgyLCxlEEmBoEoiHv

