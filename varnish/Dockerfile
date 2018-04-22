#original: https://github.com/million12/docker-varnish/blob/master/Dockerfile
FROM centos:centos7
MAINTAINER Przemyslaw Ozgo linux@ozgo.info, Marcin Ryzycki marcin@m12.io

RUN yum update -y && \
  yum install -y epel-release && \
  yum install -y varnish && \
  yum install -y libmhash-devel && \
  yum clean all

ADD start.sh /start.sh

COPY default.vcl /etc/varnish/

ENV VCL_CONFIG      /etc/varnish/default.vcl
ENV CACHE_SIZE      64m
ENV VARNISHD_PARAMS -p default_ttl=3600 -p default_grace=3600

CMD /start.sh
EXPOSE 80